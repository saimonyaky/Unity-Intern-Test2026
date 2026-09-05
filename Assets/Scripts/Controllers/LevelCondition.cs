using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCondition : MonoBehaviour
{
    public event Action ConditionCompleteEvent = delegate { };

    protected Text m_txt;

    protected bool m_conditionCompleted = false;

    private int m_moves;

    private int m_items;

    private BoardController m_board;

    public virtual void Setup(float value, Text txt)
    {
        m_txt = txt;
    }

    public virtual void Setup(GameSettings settings, Text txt, GameManager mngr, BoardController board)
    {
        m_moves = (int)settings.LevelMoves;
        m_items = (int)(settings.BoardSizeX * (settings.BoardSizeY - 1));

        m_board = board;

        m_board.OnMoveEvent += OnMove;

        m_board.OnMatchEvent += OnMatch;

        m_txt = txt;
    }

    public virtual void Setup(GameSettings settings, Text txt, GameManager mngr)
    {
        m_txt = txt;
    }

    public virtual void Setup(float value, Text txt, BoardController board)
    {
        m_txt = txt;
    }

    private void OnMove()
    {
        if (m_conditionCompleted) return;

        m_moves -= 5;

        UpdateText();

        if (m_moves <= 0)
        {
            OnConditionComplete();
        }
    }

    private void OnMatch()
    {
        if (m_conditionCompleted) return;

        m_items -= 3;
        UpdateText();
        if (m_items <= 0)
        {
            OnConditionComplete();

        }
    }
    protected virtual void UpdateText() { }

    protected void OnConditionComplete()
    {
        m_conditionCompleted = true;

        ConditionCompleteEvent();
    }

    protected virtual void OnDestroy()
    {

    }
}
