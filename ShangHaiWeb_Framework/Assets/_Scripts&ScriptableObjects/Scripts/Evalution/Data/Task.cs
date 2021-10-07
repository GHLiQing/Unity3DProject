using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Task配置文件类，将此类生成的配置文件直接拖给对应的Step的配置文件
/// 1个Task对应1个或多个Step，如果某个Step不需要Task，对应的Step不赋值Task即可
/// 在运行时会自动拷贝新的实例，不影响源文件
/// </summary>
[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Task")]
public class Task : ScriptableObject
{
    #region Field
    [SerializeField]
    //当前实例是否为拷贝体
    private bool isClone = false;
    //任务名称
    public string taskName = "";
    //任务简介
    public string taskIntroduce = "";
    [SerializeField]
    //任务状态
    private TaskState taskState;
    /// <summary>
    /// 只读属性
    /// </summary>
    public TaskState TaskState
    {
        get
        {
            return taskState;
        }
        set
        {
            if (value == TaskState.进行中)
            {
                EvalutionManager.Instance.AddDoingTask(this);
            }
            else if (value == TaskState.已完成)
            {
                EvalutionManager.Instance.AddFinishedTask(this);
            }
            taskState = value;
        }
    }

    //当前任务的总分
    public float totalScore = 100;
    //当前得分
    public float CurrentScore
    {
        get
        {
            float score = totalScore;
            for (int i = 0; i < errorList.Count; i++)
            {
                score -= errorList[i].score;
            }
            return score;
        }
    }
    //错误信息列表
    [SerializeField]
    private List<Error> errorList = new List<Error>();
    public List<Error> ErrorList
    {
        get
        {
            return errorList;
        }
    }

    //选择题表
    [SerializeField]
    private List<Question_Selection> questionList_Selection = new List<Question_Selection>();

    //图片甄别题表
    [SerializeField]
    private List<Question_ChoosePicture> questionList_ChoosePicture = new List<Question_ChoosePicture>();

    /// <summary>
    /// 动态获取选择题和图片甄别题的总和，比较耗费性能，不要持续调用
    /// </summary>
    public List<Question> allQuestionList
    {
        get
        {
            List<Question> list = new List<Question>();

            for (int i = 0; i < questionList_Selection.Count; i++)
            {
                list.Add(questionList_Selection[i]);
            }

            for (int i = 0; i < questionList_ChoosePicture.Count; i++)
            {
                list.Add(questionList_ChoosePicture[i]);
            }

            return list;
        }
    }
    #endregion
    #region Function
    #region Error
    public void AddError(ErrorType errorType, string information, float score)
    {
        if (!isClone)
        {
            Debug.Log("请拷贝实例，不要直接使用源文件");
            return;
        }
        bool isContained = false;
        for (int i = 0; i < errorList.Count; i++)
        {
            if (errorList[i].errorInformation == information)
            {
                isContained = true;
                break;
            }
        }
        if (!isContained)
        {
            errorList.Add(new Error(errorType, information, score));
        }
    }
    public void AddError(Error error)
    {
        if (!isClone)
        {
            Debug.Log("请拷贝实例，不要直接使用源文件");
            return;
        }
        bool isContained = false;
        for (int i = 0; i < errorList.Count; i++)
        {
            if (errorList[i].errorInformation == error.errorInformation)
            {
                isContained = true;
                break;
            }
        }
        if (!isContained)
        {
            errorList.Add(error);
        }
    }
    public void AddErrors(Error[] error)
    {
        for (int i = 0; i < error.Length; i++)
        {
            AddError(error[i]);
        }
    }
    public void RemoveError(string information)
    {
        if (!isClone)
        {
            Debug.Log("请拷贝实例，不要直接使用源文件");
            return;
        }
        for (int i = 0; i < errorList.Count; i++)
        {
            if (errorList[i].errorInformation == information)
            {
                errorList.Remove(errorList[i]);
                break;
            }
        }
    }
    public void RemoveAllError()
    {
        if (!isClone)
        {
            Debug.Log("请拷贝实例，不要直接使用源文件");
            return;
        }
        if (errorList != null)
            errorList.Clear();
    }
    #endregion

    #region Selection
    /// <summary>
    /// 添加选择题
    /// </summary>
    /// <param name="question"></param>
    public void AddSelectionQuestion(Question_Selection question)
    {
        if (question != null)
            questionList_Selection.Add(question);
    }
    /// <summary>
    /// 移除所有选择题
    /// </summary>
    public void RemoveAllSelectionQuestions()
    {
        questionList_Selection.Clear();
    }
    #endregion

    #region ChooseTexture
    /// <summary>
    /// 添加图片甄别题
    /// </summary>
    /// <param name="question"></param>
    public void AddChooseTextureQuestion(Question_ChoosePicture question)
    {
        if (question != null)
            questionList_ChoosePicture.Add(question);
    }
    /// <summary>
    /// 移除所有图片甄别题
    /// </summary>
    public void RemoveAllChooseTextureQuestion()
    {
        questionList_ChoosePicture.Clear();
    }
    #endregion

    #region Question_Public
    /// <summary>
    /// 移除所有题目
    /// </summary>
    public void RemoveAllQuestions()
    {
        RemoveAllSelectionQuestions();
        RemoveAllChooseTextureQuestion();
    }

    public void AddQuestionsFromXML(System.Xml.XmlNode parentNode)
    {
        //问题题目
        string title = parentNode.Attributes["title"].Value;

        string[] answers_Str = parentNode.Attributes["answers"].Value.Split('|');

        List<int> tmpList = new List<int>();

        foreach (string str in answers_Str)
        {
            if (str == "A")
                tmpList.Add(0);
            else if (str == "B")
                tmpList.Add(1);
            else if (str == "C")
                tmpList.Add(2);
            else if (str == "D")
                tmpList.Add(3);
        }

        //正确答案
        int[] answers = tmpList.ToArray();

        int score = int.Parse(parentNode.Attributes["score"].Value);

        string[] optionsText = new string[4];

        string[] optionTexturePaths = new string[4];

        for (int i = 0; i < parentNode.ChildNodes.Count; i++)
        {
            optionsText[i] = parentNode.ChildNodes[i].Attributes["text"].Value;

            optionTexturePaths[i] = parentNode.ChildNodes[i].Attributes["img"].Value;
        }

        //判断题目类别
        if (string.IsNullOrEmpty(parentNode.ChildNodes[0].Attributes["img"].Value))
        {
            //判断是否有介绍图片
            if (!string.IsNullOrEmpty(parentNode.Attributes["introduceImages"].Value))   //有介绍图片
            {
                string introduceImagePath = parentNode.Attributes["introduceImages"].Value;

                string introduceText = parentNode.Attributes["introduceText"].Value;

                AddSelectionQuestion(new Question_Selection(title, optionsText, answers, score, introduceText, introduceImagePath));
            }
            else
            {
                AddSelectionQuestion(new Question_Selection(title, optionsText, answers, score));
            }
        }
        else  //图片甄别题
        {
            AddChooseTextureQuestion(new Question_ChoosePicture(title, optionTexturePaths, optionsText, answers, score));
        }
    }
    #endregion

    #region Clone
    /// <summary>
    /// 为了运行时不影响源.asset文件，每次跳转到该步骤时都拷贝一份新的实例
    /// </summary>
    /// <returns></returns>
    public Task Clone()
    {
        isClone = true;
        Task target = Instantiate(this);
        isClone = false;
        return target;
    }
    #endregion
    #endregion
}
/// <summary>
/// 错误数据类
/// </summary>
[System.Serializable]
public class Error
{
    public Error(ErrorType errorType, string information, float score)
    {
        this.errorType = errorType;
        this.errorInformation = information;
        this.score = score;
    }

    public ErrorType errorType;

    public string errorInformation = "";

    public float score = 0;
}

public enum TaskState
{
    未开始,
    进行中,
    已完成
}

public enum ErrorType
{
    操作错误,
    答题错误,
    步骤用时,
    其他
}

/// <summary>
/// 题目状态（未答||正确||错误）
/// </summary>
public enum QuestionState
{
    未答,
    回答正确,
    回答错误
}

[System.Serializable]
//问题基类
public class Question
{
    /// <summary>
    /// 问题标题
    /// </summary>
    public string title;

    /// <summary>
    /// 问题分值
    /// </summary>
    public int score;

    /// <summary>
    /// 问题状态
    /// </summary>
    public QuestionState state;

    /// <summary>
    /// 问题答案
    /// </summary>
    public int[] answers;
}

[System.Serializable]
//（单选、多选、判断）
public sealed class Question_Selection : Question
{
    /// <summary>
    /// 问题介绍
    /// </summary>
    public string introduceText;//可能为空

    /// <summary>
    /// 图片形式的问题介绍
    /// </summary>
    public string introduceImagePath;//可能为空

    /// <summary>
    /// 问题选项
    /// </summary>
    public string[] options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title">问题标题</param>
    /// <param name="options">问题选项</param>
    /// <param name="answers">问题答案</param>
    /// <param name="introduceImages">图片形式的问题介绍</param>
    /// <param name="score">问题分值</param>
    /// <param name="introduceText">问题介绍</param>
    public Question_Selection(string title, string[] options, int[] answers, int score, string introduceText = "", string introduceImagePath = "")
    {
        this.title = title;

        this.options = options;

        this.answers = answers;

        this.introduceImagePath = introduceImagePath;

        this.score = score;

        this.introduceText = introduceText;

        state = QuestionState.未答;
    }
}

[System.Serializable]
//图片甄别题
public sealed class Question_ChoosePicture : Question
{
    /// <summary>
    /// 问题选项，图片
    /// </summary>
    public string[] optionTexturePaths;

    public string[] optionTexts;//可能为空

    public Question_ChoosePicture(string title, string[] optionTexturePaths, string[] optionTexts, int[] answers, int score)
    {
        this.title = title;

        this.optionTexturePaths = optionTexturePaths;

        this.optionTexts = optionTexts;

        this.answers = answers;

        this.score = score;

        state = QuestionState.未答;
    }
}