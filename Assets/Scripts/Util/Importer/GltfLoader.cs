using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using UnityEngine;
using UnityGLTF;

namespace Util.Importer {

public class GltfLoader : MonoBehaviour {
    private const bool MultiThreaded = false;

    public int maximumLod = 300;
    public int timeout = 8;
    public GLTFSceneImporter.ColliderType Collider = GLTFSceneImporter.ColliderType.MeshConvex;
    private AsyncCoroutineHelper asyncCoroutineHelper;

    public void Prepare() {
        asyncCoroutineHelper = gameObject.GetComponent<AsyncCoroutineHelper>() ??
                                                    gameObject.AddComponent<AsyncCoroutineHelper>();
        asyncCoroutineHelper.BudgetPerFrameInSeconds = 0.4f;
    }

    public async Task<GameObject> LoadItem(ZipArchive zipArchive, string modelFile) {
        ImportOptions importOptions = new ImportOptions {
            AsyncCoroutineHelper = asyncCoroutineHelper,
            ExternalDataLoader = null,
        };
        
        GLTFSceneImporter sceneImporter = null;
        try {
            ImporterFactory factory = ScriptableObject.CreateInstance<DefaultImporterFactory>();

            importOptions.ExternalDataLoader = new ZipLoader(zipArchive);

            sceneImporter = factory.CreateSceneImporter(
                Path.GetFileName("zip/" + modelFile),
                importOptions
            );

            sceneImporter.SceneParent = gameObject.transform;
            sceneImporter.Collider = Collider;
            sceneImporter.MaximumLod = maximumLod;
            sceneImporter.Timeout = timeout;
            sceneImporter.IsMultithreaded = MultiThreaded;
            sceneImporter.CustomShaderName = "Cel Shading/RegularV3";

            await sceneImporter.LoadSceneAsync(showSceneObj: false);

            return sceneImporter.LastLoadedScene;
        } finally {
            if (importOptions.ExternalDataLoader != null) {
                sceneImporter?.Dispose();
                sceneImporter = null;
                importOptions.ExternalDataLoader = null;
            }
        }
    }
}

}