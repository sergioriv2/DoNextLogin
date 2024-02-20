using System.Reflection;
using System.Text;

namespace ServerlessLogin.Helpers
{
    public class ResourcesHelper
    {
        public string ReadResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            string resourceFullName = $"ServerlessLogin.{resourceName}";

            // Accede al recurso embebido
            using (Stream stream = assembly.GetManifestResourceStream(resourceFullName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"El recurso {resourceFullName} no se encontró. Asegúrate de que el nombre del recurso sea correcto.");
                }
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    // Lee el contenido del archivo
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
