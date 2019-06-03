
namespace ZLAN.Command.Abs.Observable
{
    public class MessageContext
    {
        public string Key { get; set; }

        public object Message { get; set; }

        public IParameter Parameter { get; set; }

        public object Result { get; set; }
    }
}
