namespace Technolithic
{
    public interface IApproachable
    {
        bool IsDestroyed { get; }

        Tile GetApproachableTile(CreatureCmp creature);

        Room GetRoom();
    }
}
