namespace TesterStrategy.BLL.Interfaces
{
    public interface IStockCommunity
    {
        void RegisterTrader(ITrader trader);

        void Notify();
    }
}
