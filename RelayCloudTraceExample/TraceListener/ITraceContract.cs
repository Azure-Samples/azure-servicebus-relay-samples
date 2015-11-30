
using System.ServiceModel;

namespace RelaySamples
{
    [ServiceContract(Name = "tct", Namespace = "", SessionMode = SessionMode.Allowed)]
    public interface ITraceContract
    {
        [OperationContract(IsOneWay = true, Name = "Write1")]
        void Write(string message);

        [OperationContract(IsOneWay = true, Name = "Write2")]
        void Write(string message, string category);

        [OperationContract(IsOneWay = true, Name = "WriteLine1")]
        void WriteLine(string message);

        [OperationContract(IsOneWay = true, Name = "WriteLine2")]
        void WriteLine(string message, string category);

        [OperationContract(IsOneWay = true, Name = "Fail1")]
        void Fail(string message);

        [OperationContract(IsOneWay = true, Name = "Fail2")]
        void Fail(string message, string detailMessage);
    }

    public interface ITraceChannel : ITraceContract, IClientChannel { }
}