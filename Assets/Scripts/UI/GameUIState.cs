// Base state class

using UI;

public abstract class GameUIState : IGameUIState
{
    protected GameUIController controller;

    public GameUIState(GameUIController controller)
    {
        this.controller = controller;
    }

    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void Update();
}
