using Game339.Shared.Models;
using Game339.Shared.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Runtime
{
    public class BadGuyReverseItView : ObserverMonoBehaviour
    {
        [SerializeField] private Button button;

        protected override void Subscribe()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        protected override void Unsubscribe()
        {
            button.onClick.RemoveListener(OnButtonClick);
        }

        private static void OnButtonClick()
        {
            var gameState = ServiceResolver.Resolve<GameState>();
            var stringService = ServiceResolver.Resolve<IStringService>();
            // var reversedString = stringService.Reverse(gameState.GoodGuy.Name.Value);
            var reversedString = stringService.ReverseWords(gameState.GoodGuy.Name.Value);
            gameState.GoodGuy.Name.Value = reversedString;
        }
    }
}
