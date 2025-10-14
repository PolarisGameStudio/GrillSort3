
using Cysharp.Threading.Tasks;

public interface IHomeProcess
{
    public UniTask<bool> ProcessTask();
}