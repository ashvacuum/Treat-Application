namespace UI
{
    public class GameState : GameUIState
    {
        public GameState(GameUIController controller) : base(controller) { }

        public override void OnEnter()
        {
            controller.ShowGamePanel(true);
            // Setup in-game UI elements, listeners, etc.
        }

        public override void OnExit()
        {
            controller.ShowGamePanel(false);
            // Clean up in-game UI elements, listeners, etc.
        }

        public override void Update()
        {
            // Handle in-game state updates
        }
    }
}