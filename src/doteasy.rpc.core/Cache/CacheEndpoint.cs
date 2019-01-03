namespace DotEasy.Rpc.Core.Cache
{
    public abstract class CacheEndpoint
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public abstract override string ToString();

        public override bool Equals(object obj)
        {
            var model = obj as CacheEndpoint;
            if (model == null)
                return false;

            if (obj.GetType() != GetType())
                return false;

            return model.ToString() == ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator ==(CacheEndpoint model1, CacheEndpoint model2)
        {
            return Equals(model1, model2);
        }

        public static bool operator !=(CacheEndpoint model1, CacheEndpoint model2)
        {
            return !Equals(model1, model2);
        }
    }
}