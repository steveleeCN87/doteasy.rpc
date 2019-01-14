namespace DotEasy.Rpc.Core.Cache.Caching
{
    public abstract class CachingEndpoint
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public abstract override string ToString();

        public override bool Equals(object obj)
        {
            var model = obj as CachingEndpoint;
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

        public static bool operator ==(CachingEndpoint model1, CachingEndpoint model2)
        {
            return Equals(model1, model2);
        }

        public static bool operator !=(CachingEndpoint model1, CachingEndpoint model2)
        {
            return !Equals(model1, model2);
        }
    }
}