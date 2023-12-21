using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace Metelab.PeakGameCase
{
    /// Sorry about that class :(
    public class GameOverPanel : MetePanel
    {
  
        public TextMeshProUGUI TextResult;
        public RectTransform ButtonTryAgain;
        public RectTransform ButtonOpenLevelSelection;
        private bool IsFistTime;
        private bool isOpen;
        public void OpenLevelSelectionPanel()
        {
            HidePanel();
            PanelControllerMainScene.current.ShowPanel(PanelNames.LevelSelection);
        }

        public void TryAgain()
        {
            HidePanel();
            PanelControllerMainScene.current.GetPanel<GamePanel>(PanelNames.GamePanel).StartGame();
        }

        private EndGameResult lastResult;
        public void StartPanelForResult( EndGameResult result)
        {
            isOpen = false;
            lastResult = result;
            IsFistTime = PlayerPrefs.GetInt(nameof(IsFistTime), 0) == 0;
            ShowPanel();
            ButtonTryAgain.localScale = Vector3.zero;
            ButtonOpenLevelSelection.localScale = Vector3.zero;
            TextResult.text = string.Empty;

            StopAllCoroutines();

            if (result == EndGameResult.LOSE)
                StartCoroutine(LoseTextType());
            else
                StartCoroutine(WinTextType());
        }

       

        private void Update()
        {
            if (!IsFistTime && Input.anyKey && !isOpen)
            {
               
                StopAllCoroutines();

                if (lastResult == EndGameResult.WIN)
                {
                    TextResult.text = "Congratulations";
                }
                else
                {
                    TextResult.text = "Never give up!";
                }

                
                ButtonTryAgain.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
                ButtonOpenLevelSelection.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
                isOpen = true;
            }
        }

        public IEnumerator WinTextType()
        {
            string text = "WIN\nTry other levels";

            for (int i = 0; i <= text.Length; i++)
            {
                TextResult.text = text[..i];
                yield return new WaitForSeconds(0.1f);
            }

            ButtonTryAgain.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            ButtonOpenLevelSelection.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            isOpen = true;
        }

        public IEnumerator LoseTextType()
        {
            string text = "LOSE...";

            for (int i = 0;i<= text.Length; i++)
            {
                TextResult.text = text[..i];
                yield return new WaitForSeconds(0.3f);
            }

            yield return new WaitForSeconds(1f);

            for (int i = text.Length-1; i >= 0 ; i--)
            {
                TextResult.text = text[..i];
                yield return new WaitForSeconds(0.05f);
            }

            text = "YOU WILL WIN\nIN LOVE <3\nDont Worry\nTry Again ";

            for (int i = 0; i <= text.Length; i++)
            {
                TextResult.text = text[..i];
                yield return new WaitForSeconds(0.05f);
            }

            ButtonTryAgain.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            ButtonOpenLevelSelection.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);

            isOpen = true;
            PlayerPrefs.SetInt(nameof(IsFistTime), 1);
        }

    }

}
