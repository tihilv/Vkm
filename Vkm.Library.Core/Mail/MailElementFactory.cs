using Vkm.Api.Element;
using Vkm.Api.Identification;

namespace Vkm.Library.Mail
{
    public class MailElementFactory: IElementFactory
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.MailElement.Factory");

        public Identifier Id => Identifier;
        
        public string Name => "Mail Controls";

        public IElement CreateElement(Identifier id)
        {
            return new MailElement(id);
        }
    }
}