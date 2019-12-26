using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    public PieceType type;
    private Piece currentPiece;

    public void Spawn ()
    {
        int amountObj = 0;
        switch (type)
        {
            case PieceType.jump:
                amountObj = LevelManager.Instance.Jumps.Count;
                break;

            case PieceType.longblock:
                amountObj = LevelManager.Instance.LongBlocks.Count;
                break;

            case PieceType.ramp:
                amountObj = LevelManager.Instance.Ramps.Count;
                break;

            case PieceType.slide:
                amountObj = LevelManager.Instance.Slides.Count;
                break;
        }
        currentPiece = LevelManager.Instance.GetPiece(type, Random.Range(0, amountObj));
        currentPiece.gameObject.SetActive(true);
        currentPiece.transform.SetParent(transform, false);

    }

    public void Despawn ()
    {
        currentPiece.gameObject.SetActive(false);
    }
}
