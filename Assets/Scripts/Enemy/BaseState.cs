
public abstract class BaseState
{
    protected Enemy currentEnemy;
    public abstract void OnEnter(Enemy Enemy);
    public abstract void LogicUpdate();
    public abstract void PhysicsUpdate();
    public abstract void OnExit();
}
