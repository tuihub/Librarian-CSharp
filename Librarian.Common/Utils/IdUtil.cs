using IdGen;

namespace Librarian.Common.Utils
{
    public static class IdUtil
    {
        static int GeneratorId;
        static IdGenerator Generator;
        static IdUtil()
        {
            GeneratorId = GlobalContext.SystemConfig.GeneratorId;
            Generator = new IdGenerator(GeneratorId);
        }
        public static long NewId()
        {
            return Generator.CreateId();
        }
    }
}
