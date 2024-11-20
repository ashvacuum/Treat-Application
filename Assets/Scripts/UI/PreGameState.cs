// PreGame State

namespace UI
{
    public class PreGameState : GameUIState
    {
        public PreGameState(GameUIController controller) : base(controller) { }

        public override void OnEnter()
        {
            controller.ShowPreGamePanel(true);
            controller.ShowGamePanel(false);
            controller.ShowPostGamePanel(false);
            // Setup pre-game UI elements, listeners, etc.
            
        }

        public override void OnExit()
        {
            controller.ShowPreGamePanel(false);
            // Clean up pre-game UI elements, listeners, etc.
            controller.ShowLeaderboardsPanel(false);
        }

        public override void Update()
        {
            // Handle pre-game state updates
        }
    }
}