using System.IO;

namespace ModuleLauncher.Re.Models.Locators.Dependencies
{
    public class Dependency
    {
        /// <summary>
        /// Name of this dependence item
        /// e.g. jna-3.4.0.jar(library) or 92750c5f93c312ba9ab413d546f32190c56d6f1f(asset)
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// A FileInfo wrapper of full path of the dependence item
        /// e.g. C:\Users\ahpx\AppData\Roaming\.minecraft\libraries\net\java\dev\jna\jna\3.4.0\jna-3.4.0.jar
        /// or
        /// C:\Users\ahpx\AppData\Roaming\.minecraft\assets\objects\92\92750c5f93c312ba9ab413d546f32190c56d6f1f
        /// </summary>
        public FileInfo File { get; set; }
        
        /// <summary>
        /// The relative download url of the dependence item
        /// e.g.
        /// net/java/dev/jna/jna/3.4.0/jna-3.4.0.jar
        /// or
        /// 92/92750c5f93c312ba9ab413d546f32190c56d6f1f
        /// </summary>
        public string RelativeUrl { get; set; }
    }
}