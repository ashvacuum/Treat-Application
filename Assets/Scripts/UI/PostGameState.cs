namespace UI
{
    public class PostGameState : GameUIState
    {
        public PostGameState(GameUIController controller) : base(controller) { }

        public override void OnEnter()
        {
            controller.ShowPostGamePanel(true);
            controller.ShowPreGamePanel(false);
            controller.ShowGamePanel(false);
            // Setup post-game UI elements, listeners, etc.
        }

        public override void OnExit()
        {
            controller.ShowPostGamePanel(false);
            // Clean up post-game UI elements, listeners, etc.
        }

        public override void Update()
        {
            // Handle post-game state updates
        }
    }
}