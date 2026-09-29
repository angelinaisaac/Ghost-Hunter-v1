using System;
using UnityEngine;
using System.Collections.Generic;

public class LurePuzzleManager : MonoBehaviour
{
    private HashSet<Lure1Functionality> completedHolders = new HashSet<Lure1Functionality>();
    [SerializeField] private GameObject puzzleCompleteObject;
    [SerializeField] private AudioClip objectReveal;
    [SerializeField] private SFXPlayer sfxPlayer;
    private bool puzzleCompleted = false;

    public void HolderFilled(Lure1Functionality holder)
    {
        if (puzzleCompleted)
            return;

        //count each holder only once
        if (!completedHolders.Add(holder))
            return;

        //if 3 holders are filled the puzzle is complete
        if (completedHolders.Count >= 3)
        {
            PuzzleComplete();
        }
    }

    private void PuzzleComplete()
    {
        if (puzzleCompleted)
            return;

        puzzleCompleted = true;
        //grant the player their item
        sfxPlayer.PlaySFX(objectReveal);
        puzzleCompleteObject.SetActive(true);
    }
}
