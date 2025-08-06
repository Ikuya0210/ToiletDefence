using UnityEngine;

namespace DIPractice
{
    public class GamePresenter : VContainer.Unity.IStartable
    {
        readonly HelloWorldService helloWorldService;
        readonly HelloCanvas helloCanvas;

        public GamePresenter(HelloWorldService helloWorldService, HelloCanvas helloCanvas)
        {
            this.helloWorldService = helloWorldService;
            this.helloCanvas = helloCanvas;
        }

        void VContainer.Unity.IStartable.Start()
        {
            helloCanvas.helloButton.onClick.AddListener(() => helloWorldService.Hello());
        }
    }
}
