using UnityEngine;
using DG.Tweening;

public class InputManager : MonoBehaviour
{
    private Screw selectedScrew;
    private bool isAnimating = false;

    private void Update()
    {
        if (isAnimating || !Input.GetMouseButtonDown(0)) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit.collider != null)
        {
            Screw clickedScrew = hit.collider.GetComponent<Screw>();
            if (clickedScrew != null)
            {
                HandleScrewClick(clickedScrew);
            }
            else
            {
                DeselectScrew();
            }
        }
        else
        {
            DeselectScrew();
        }
    }

    private void HandleScrewClick(Screw clickedScrew)
    {
        if (selectedScrew == null)
        {
            selectedScrew = clickedScrew;
            selectedScrew.Highlight(true);
        }
        else
        {
            if (selectedScrew != clickedScrew)
            {
                var (sequence, count) = selectedScrew.GetMovableSequence();
                if (count > 0 && clickedScrew.CanAddNuts(count))
                {
                    var targetNuts = clickedScrew.GetNuts();
                    bool isValidMove = targetNuts.Count == 0 ||
                        (targetNuts[targetNuts.Count - 1].ColorIndex == sequence[0].ColorIndex);
                    if (isValidMove)
                    {
                        isAnimating = true;
                        var movedNuts = selectedScrew.RemoveTopSequence(count);
                        clickedScrew.AddNuts(movedNuts);
                        DOTween.Sequence().AppendInterval(0.5f).OnComplete(() =>
                        {
                            GameManager.Instance.CheckWinCondition();
                            isAnimating = false;
                        });
                    }
                    else
                    {
                        Debug.Log("Invalid move: Target screw not empty or colors don't match");
                    }
                }
                else
                {
                    Debug.Log($"Cannot move: No nuts to move or target screw full (count: {count}, canAdd: {clickedScrew.CanAddNuts(count)})");
                }
            }
            else
            {
                Debug.Log("Clicked same screw, deselecting");
            }
            DeselectScrew();
        }
    }

    private void DeselectScrew()
    {
        if (selectedScrew != null)
        {
            selectedScrew.Highlight(false);
            selectedScrew = null;
        }
    }
}