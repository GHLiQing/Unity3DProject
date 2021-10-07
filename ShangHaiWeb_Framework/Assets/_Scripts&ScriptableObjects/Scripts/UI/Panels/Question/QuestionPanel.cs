using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
/// <summary>
/// 注意实例化多个此组件时，保持每个名字不同，否则注册会出现问题
/// </summary>
public class QuestionPanel : Panel
{
    public Question question;

    Question_Selection selectionQuestion;
    Question_ChoosePicture choosePictureQuestion;

    private UnityAction<bool> answerCallBack;


    int[] answerList;                  //当前问题的正确答案列表
    private List<int> selectedList = new List<int>();

    private List<QuestionOptionItem> optionItemList = new List<QuestionOptionItem>();   //每个选项 

    public bool IsCorrect = false;                 //答题结果

    private GameObject BG;
    public GameObject selectionPanelPrefab;       //选择题面板预制体
    public GameObject choosePicturePrefab;        //图片甄别题面板

    public GameObject selectionOptionPrefabs;    //选择题选项预制体
    public GameObject ImageOptionPrefab;         //图片甄别题选项预制体

    private Transform selectOptionParent;       //选择题选项的父物体
    private Transform pictureOptionParent;       //图片甄别题选项的父物体

    private string answerText;

    private void Start()
    {
        IntToChar(3);
    }

    private void InitQuestion()
    {
        if (question == null)
            return;
        ResetQuestion();
        SelectQuestionType(question);
    }

    public void InitQuestion(Question question, UnityAction<bool> answerCallBack)
    {
        this.question = question;
        this.answerCallBack = answerCallBack;
        InitQuestion();
    }

    /// <summary>
    /// 在流程中对确定按钮事件进行注册
    /// </summary>
    /// <param name="answerCallBack"></param>
    public void SetEvent_AnswerCallBack(UnityAction<bool> answerCallBack)
    {
        this.answerCallBack = answerCallBack;
    }

    /// <summary>
    /// 重置数据
    /// </summary>
    private void ResetQuestion()
    {
        answerList = new int[0];
        IsCorrect = false;
        selectedList.Clear();
    }




    #region 从allQuestionList中按照索引取得题目


    /// <summary>
    /// 区分题目是选择题还是图片甄别题
    /// </summary>
    /// <param name="index"></param>
    private void SelectQuestionType(Question question)
    {
        System.Type type = question.GetType();
        if (type == typeof(Question_Selection))
        {
            CreatePanel(selectionPanelPrefab);
            selectOptionParent = BG.transform.Find("Options").transform;
            selectionQuestion = (Question_Selection)question;
            SelectionQuestion(selectionQuestion);
        }
        else if (type == typeof(Question_ChoosePicture))
        {
            CreatePanel(choosePicturePrefab);
            pictureOptionParent = BG.transform.Find("Options").transform;
            choosePictureQuestion = (Question_ChoosePicture)question;
            ChoosePictureQuestion(choosePictureQuestion);
        }
        BG.transform.Find("Confirm/ConfirmButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (answerCallBack == null)
            {
                Debug.Log("未注册事件");
                return;
            }
            if (isAnswered)
            {
                Destroy(this.transform.gameObject);
                answerCallBack(true);
                return;
            }
            bool istrue = ConfirmButton();
            MyUtilities.DelayToDo(2.5f, () => { answerCallBack((istrue)); });
        });
        SetMyRectTransform();
    }
    private bool isAnswered = false;
    #endregion



    /// <summary>
    /// 根据题目类型创建问题面板
    /// </summary>
    /// <param name="panelPrefab"></param>
    /// 
    void CreatePanel(GameObject panelPrefab)
    {
        GameObject panel = Instantiate(panelPrefab,transform,false);
        panel.transform.name = "BG";
        BG = panel;
    }


    public override void SetPanelDisplay(bool display)
    {
        isDisplay = display;
        if (myRectTransfrom != null)
        {
            //自定义方式开关相关UI
            myRectTransfrom.gameObject.SetActive(display);
        }
    }

    void SetMyRectTransform()
    {
        if (registeMyselfOnAwake)
        {
            RegisteMyself(gameObject.name);
        }
        if (this.transform.childCount > 0)
        {
            myRectTransfrom = this.transform.GetChild(0).GetComponent<RectTransform>();
        }

    }


    #region 选择题初始化
    /// <summary>
    /// 选择题的初始化
    /// </summary>
    /// <param name="selectionQuestion"></param>
    void SelectionQuestion(Question_Selection selectionQuestion)
    {

        AddSelectionOptionItem(selectionQuestion);

        answerList = new int[selectionQuestion.answers.Length];
        answerList = selectionQuestion.answers;

        for (int i = 0; i < selectOptionParent.childCount; i++)
        {
            optionItemList.Add(selectOptionParent.transform.GetChild(i).GetComponent<QuestionOptionItem>());
            selectOptionParent.transform.GetChild(i).GetComponent<QuestionOptionItem>().index = i;

        }
        transform.GetComponent<QuestionPanel>().SetQuestionItemClickEvent((i, isOn) =>
        {
            OptionState(i, isOn);
            Debug.Log(i + " ::: " + isOn);
        });
    }

    /// <summary>
    /// 根据问题选项，生产UI选项
    /// </summary>
    /// <param name="selection"></param>
    void AddSelectionOptionItem(Question_Selection selection)
    {
        for (int i = 0; i < selection.options.Length; i++)
        {
            GameObject item = Instantiate(selectionOptionPrefabs, BG.transform.Find("Options").transform, false);
            item.transform.Find("Label").GetComponent<Text>().text = IntToChar(i) + ":" + selection.options[i];
        }
        BG.transform.Find("Title/text").GetComponent<Text>().text = selection.title;
        SetSingleOrMultipleQuestion(selection, BG.transform.Find("Options").transform.gameObject);
        LoadDetailIntroduce(selection);
    }

    /// <summary>
    /// 加载细节介绍，可以是文字也可以是图片
    /// </summary>
    void LoadDetailIntroduce(Question_Selection selection)
    {
        if (selection.introduceText != "")
        {
            BG.transform.Find("TitleDetail/TitleDetailText").gameObject.GetComponent<Text>().text = selection.introduceText;
        }
        else
        {
            BG.transform.Find("TitleDetail/TitleDetailText").gameObject.SetActive(false);
        }
        if (selection.introduceImagePath != "")
        {
            LoadAssetManager.Instance.Load_WWW(IPathTools.GetApplicationDataPath() + selection.introduceImagePath, (www) =>
            {
                BG.transform.Find("TitleDetail/TitleDetailImage").gameObject.GetComponent<RawImage>().texture = www.texture;
            }, null);
        }
        else
        {
            BG.transform.Find("TitleDetail/TitleDetailImage").gameObject.SetActive(false);
        }

    }

    #endregion


    #region 图片甄别题初始化
    /// <summary>
    /// 图片甄别题的初始化
    /// </summary>
    /// <param name="choosePicture"></param>
    void ChoosePictureQuestion(Question_ChoosePicture choosePicture)
    {

        print("图片甄别题暂未完成");
        AddImageOptionItem(choosePicture);

        answerList = new int[choosePicture.answers.Length];
        answerList = choosePicture.answers;
        print(answerList.Length);

        for (int i = 0; i < pictureOptionParent.childCount; i++)
        {
            optionItemList.Add(pictureOptionParent.transform.GetChild(i).GetComponent<QuestionOptionItem>());
            pictureOptionParent.transform.GetChild(i).GetComponent<QuestionOptionItem>().index = i;

        }
        transform.GetComponent<QuestionPanel>().SetQuestionItemClickEvent((i, isOn) =>
        {
            OptionState(i, isOn);
        });
    }
    /// <summary>
    /// 根据问题选项，生产UI选项
    /// </summary>
    /// <param name="selection"></param>
    void AddImageOptionItem(Question_ChoosePicture choosePicture)
    {

        for (int i = 0; i < choosePicture.optionTexturePaths.Length; i++)
        {
            GameObject item = Instantiate(ImageOptionPrefab, BG.transform.Find("Options").transform, false);
            item.transform.Find("Text").GetComponent<Text>().text = IntToChar(i) + ":" + choosePicture.optionTexts[i];
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            LoadAssetManager.Instance.Load_WWW(IPathTools.GetApplicationDataPath() + choosePicture.optionTexturePaths[i], (www) =>
            {
                item.GetComponent<RawImage>().texture = www.texture;

            }, (progress) =>
            {
            });
        }
        SetSingleOrMultipleQuestion(choosePicture, BG.transform.Find("Options").transform.gameObject);
        BG.transform.Find("Title/text").GetComponent<Text>().text = choosePicture.title;

    }

    #endregion


    #region 结果判断
    /// <summary>
    /// 设置问题选项的点击事件
    /// </summary>
    /// <param name="OnStepToolItemClick"></param>
    private void SetQuestionItemClickEvent(UnityAction<int, bool> OnOptionItemClick)
    {
        for (int i = 0; i < optionItemList.Count; i++)
        {
            optionItemList[i].QuestionItemClick = OnOptionItemClick;
        }
    }

    /// <summary>
    /// 将选择的选项添加到选项列表中，用于最后的判断
    /// </summary>
    /// <param name="index"></param>
    /// <param name="isOn"></param>
    void OptionState(int index, bool isOn)
    {
        if (isOn)
        {
            selectedList.Add(index);
        }
        else
        {
            selectedList.Remove(index);
        }
    }

    /// <summary>
    /// 判断问题正确错误，遍历选项列表和正确答案列表，进行对比，如果选项完全相同，就正确
    /// </summary>
    /// <returns></returns>
    public bool ConfirmButton()
    {
        IsCorrect = true;
        if (selectedList.Count == answerList.Length)
        {
            for (int i = 0; i < answerList.Length; i++)
            {
                if (!selectedList.Contains(answerList[i]))
                {
                    IsCorrect = false;
                }
            }
        }
        else
        {
            IsCorrect = false;
        }

        ReturnTaskResult(IsCorrect);
        return IsCorrect;

    }

    /// <summary>
    /// 设置当前问题的结果状态
    /// </summary>
    /// <param name="isRight"></param>
    private void ReturnTaskResult(bool isRight)
    {

        if (question.state == QuestionState.未答)
        {
            isAnswered = true;
            if (isRight)
            {
                AnswerRightEvent();
            }
            else
            {
                AnswerWrongEvent();
            }
            for (int i = 0; i < BG.transform.Find("Options").transform.childCount; i++)
            {
                BG.transform.Find("Options").transform.GetChild(i).GetComponent<Toggle>().interactable = false;
            }
        }
        else
        {
            MyUtilities.DelayToDo(2, () =>
            {
                Destroy(this.transform.gameObject);
            });
        }
        print(question.state);
    }


    void AnswerRightEvent()
    {
        question.state = QuestionState.回答正确;
        transform.Find("BG/Correct").transform.gameObject.SetActive(true);
        transform.Find("BG/Wrong").transform.gameObject.SetActive(false);
        BG.transform.Find("Confirm/ConfirmButton").GetComponent<Button>().interactable = false;
        MyUtilities.DelayToDo(2, () =>
        {
            Destroy(this.transform.gameObject);
        });
    }
    void AnswerWrongEvent()
    {
        question.state = QuestionState.回答错误;
        transform.Find("BG/Correct").transform.gameObject.SetActive(false);
        transform.Find("BG/Wrong").transform.gameObject.SetActive(true);
        //BG.transform.Find("Confirm/ConfirmButton").GetComponent<Button>().interactable = false;

        for (int i = 0; i < answerList.Length; i++)  //将答案数字转化成相应的字母显示出来
        {
            answerText += IntToChar(answerList[i]) + " ";
        }
        BG.transform.Find("AnswerText").transform.gameObject.SetActive(true);
        BG.transform.Find("AnswerText").GetComponent<Text>().text = "正确选项是：" + answerText;

        //this.DelayToDo(3, () =>
        //{
        //    BG.transform.Find("Confirm/ConfirmButton").GetComponent<Button>().interactable = true;
        //});
    }

    #endregion


    void DestroyPanel()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// 设置单选题时只能选择一个选项（使用ToggleGroup控制）
    /// </summary>
    /// <param name="question"></param>
    /// <param name="options"></param>
    void SetSingleOrMultipleQuestion(Question question, GameObject options)
    {
        if (question.answers.Length == 1)
        {
            options.AddComponent<ToggleGroup>();
            for (int i = 0; i < options.transform.childCount; i++)
            {
                options.transform.GetChild(i).gameObject.GetComponent<Toggle>().group = options.GetComponent<ToggleGroup>();
            }
        }

    }


    string IntToChar(int index)
    {
        int a = 65 + index;
        char zm = (char)a;
        string s = zm.ToString();
        return s;

    }

}
