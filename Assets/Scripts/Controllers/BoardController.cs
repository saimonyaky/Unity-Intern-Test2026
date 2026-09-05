using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public event Action OnMoveEvent = delegate { };

    public event Action OnMatchEvent = delegate { };

    public bool IsBusy { get; private set; }

    private Board m_board;

    private GameManager m_gameManager;

    // private bool m_isDragging;

    private Camera m_cam;

    private Collider2D m_hitCollider;

    private GameSettings m_gameSettings;

    private List<Cell> m_potentialMatch;

    private List<Cell> m_bottomCells;

    private int m_bottomCount;

    private float m_timeAfterFill;

    private bool m_hintIsShown;

    private bool m_gameOver;

    public void StartGame(GameManager gameManager, GameSettings gameSettings)
    {
        m_gameManager = gameManager;

        m_gameSettings = gameSettings;

        m_gameManager.StateChangedAction += OnGameStateChange;

        m_cam = Camera.main;

        m_board = new Board(this.transform, gameSettings);
        m_bottomCells = m_board.GetBottomCells();
        m_bottomCount = 0;

        Fill();
    }

    private void Fill()
    {
        m_board.Fill();
        FindMatchesAndCollapse();
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_STARTED:
                IsBusy = false;
                break;
            case GameManager.eStateGame.PAUSE:
                IsBusy = true;
                break;
            case GameManager.eStateGame.GAME_OVER:
                m_gameOver = true;
                StopHints();
                break;
        }
    }


    public void Update()
    {
        if (m_gameOver) return;
        if (IsBusy) return;

        if (!m_hintIsShown)
        {
            m_timeAfterFill += Time.deltaTime;
            if (m_timeAfterFill > m_gameSettings.TimeForHint)
            {
                m_timeAfterFill = 0f;
                ShowHint();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                m_hitCollider = hit.collider;

                Cell cell = m_hitCollider.GetComponent<Cell>();
                if (!cell.IsEmpty)
                {
                    IsBusy = true;

                    if (m_bottomCount >= m_bottomCells.Count)
                    {
                        return;
                    }
                    else
                    {
                        FindMatchesAndSort(cell);
                        m_bottomCount++;
                    }
                }

                // Cell c2 = m_bottomCells[m_bottomCount];

                // m_board.Move(c1, c2);

                // m_bottomCount++;
                ResetRayCast();
            }
            else
            {
                ResetRayCast();
            }
        }

        // if (Input.GetMouseButtonUp(0))
        // {
        //     ResetRayCast();
        // }

        // if (Input.GetMouseButton(0) && m_isDragging)
        // {
        //     var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        //     if (hit.collider != null)
        //     {
        //         if (m_hitCollider != null && m_hitCollider != hit.collider)
        //         {
        //             StopHints();

        //             Cell c1 = m_hitCollider.GetComponent<Cell>();
        //             Cell c2 = hit.collider.GetComponent<Cell>();
        //             if (AreItemsNeighbor(c1, c2))
        //             {
        //                 IsBusy = true;
        //                 SetSortingLayer(c1, c2);
        //                 m_board.Swap(c1, c2, () =>
        //                 {
        //                     FindMatchesAndCollapse(c1, c2);
        //                 });

        //                 ResetRayCast();
        //             }
        //         }
        //     }
        //     else
        //     {
        //         ResetRayCast();
        //     }
        // }
    }

    private void ResetRayCast()
    {
        // m_isDragging = false;
        m_hitCollider = null;
    }

    private void FindMatchesAndSort(Cell cell)
    {
        // if (cell1.Item is BonusItem)
        // {
        //     cell1.ExplodeItem();
        //     StartCoroutine(ShiftDownItemsCoroutine());
        // }
        // else if (cell2.Item is BonusItem)
        // {
        //     cell2.ExplodeItem();
        //     StartCoroutine(ShiftDownItemsCoroutine());
        // }
        // else
        // {
        // Cell newCell = cell;
        int index = m_bottomCount;
        List<Cell> matches = new List<Cell>();
        for (int i = 0; i < m_bottomCount; i++)
        {
            if (m_board.CheckMatches(cell, m_bottomCells[i]))
            {
                index = i + 1;
                matches.Add(m_bottomCells[i]);
            }
        }
        for (int i = m_bottomCount; i > index; i--)
        {
            m_board.Move(m_bottomCells[i - 1], m_bottomCells[i], () =>
                {
                    IsBusy = false;
                });
        }

        // newCell = m_bottomCells[index];
        matches.Add(m_bottomCells[index]);
        m_board.Move(cell, m_bottomCells[index], () =>
                {
                    IsBusy = false;
                    if (matches.Count >= m_gameSettings.MatchesMin)
                    {
                        Debug.Log($"da an {matches}");
                        CollapseMatches(matches);
                    }
                    else if (m_bottomCount >= 5)
                    {
                        OnMoveEvent();
                    }
                });
        // Debug.Log($"{m_bottomCells[index].NeighbourLeft}");
        // List<Cell> cells = GetMatches(m_bottomCells[index]);
        // List<Cell> cells2 = GetMatches(cell2);

        // List<Cell> matches = new List<Cell>();
        // matches.AddRange(cells);
        // // matches.AddRange(cells2);
        // matches = matches.Distinct().ToList();
        // Debug.Log($"{matches.Count}");


        // }
    }

    private void FindMatchesAndCollapse()
    {
        List<Cell> matches = m_board.FindFirstMatch();

        if (matches.Count > 0)
        {
            CollapseMatches(matches);
        }
        else
        {
            m_potentialMatch = m_board.GetPotentialMatches();
            if (m_potentialMatch.Count > 0)
            {
                IsBusy = false;

                m_timeAfterFill = 0f;
            }
            else
            {
                //StartCoroutine(RefillBoardCoroutine());
                // StartCoroutine(ShuffleBoardCoroutine());
            }
        }
    }

    // private List<Cell> GetMatches(Cell cell)
    // {
    //     List<Cell> listHor = m_board.GetHorizontalMatches(cell);
    //     if (listHor.Count < m_gameSettings.MatchesMin)
    //     {
    //         listHor.Clear();
    //     }

    //     List<Cell> listVert = m_board.GetVerticalMatches(cell);
    //     if (listVert.Count < m_gameSettings.MatchesMin)
    //     {
    //         listVert.Clear();
    //     }

    //     return listHor.Concat(listVert).Distinct().ToList();
    // }

    private List<Cell> GetMatches(Cell cell)
    {
        Debug.Log($"{cell.NeighbourLeft}");
        List<Cell> list = m_board.GetMatches(cell);
        if (list.Count < m_gameSettings.MatchesMin)
        {
            list.Clear();
        }

        return list.Distinct().ToList();
    }

    private void CollapseMatches(List<Cell> matches)
    {
        for (int i = 0; i < matches.Count; i++)
        {
            matches[i].ExplodeItem();
            m_bottomCount--;
        }
        OnMatchEvent();

        // if (matches.Count > m_gameSettings.MatchesMin)
        // {
        //     m_board.ConvertNormalToBonus(matches, cellEnd);
        // }

        StartCoroutine(ShiftDownItemsCoroutine());
    }

    private IEnumerator ShiftDownItemsCoroutine()
    {
        m_board.ShiftDownItems();

        yield return new WaitForSeconds(0.2f);

        // m_board.FillGapsWithNewItems();

        // yield return new WaitForSeconds(0.2f);

        // FindMatchesAndCollapse();
    }

    private IEnumerator RefillBoardCoroutine()
    {
        m_board.ExplodeAllItems();

        yield return new WaitForSeconds(0.2f);

        m_board.Fill();

        yield return new WaitForSeconds(0.2f);

        FindMatchesAndCollapse();
    }

    // private IEnumerator ShuffleBoardCoroutine()
    // {
    //     m_board.Shuffle();

    //     yield return new WaitForSeconds(0.3f);

    //     FindMatchesAndCollapse();
    // }


    private void SetSortingLayer(Cell cell1)
    {
        if (cell1.Item != null) cell1.Item.SetSortingLayerHigher();
        // if (cell2.Item != null) cell2.Item.SetSortingLayerLower();
    }

    private bool AreItemsNeighbor(Cell cell1, Cell cell2)
    {
        return cell1.IsNeighbour(cell2);
    }

    internal void Clear()
    {
        m_board.Clear();
    }

    private void ShowHint()
    {
        m_hintIsShown = true;
        foreach (var cell in m_potentialMatch)
        {
            cell.AnimateItemForHint();
        }
    }

    private void StopHints()
    {
        m_hintIsShown = false;
        foreach (var cell in m_potentialMatch)
        {
            cell.StopHintAnimation();
        }

        m_potentialMatch.Clear();
    }
}
