using AssetRipper.Core.Interfaces;
using AssetRipper.Core.Parser.Files.SerializedFiles;
using AssetRipper.Core.Project.Collections;

namespace AssetRipper.Core.Project.Exporters
{
	/// <summary>
	/// The name of this class is somewhat misleading.
	/// This class handles both scene files and prefabs.
	/// </summary>
	public class SceneYamlExporter : YamlExporterBase
	{
		public override bool IsHandle(IUnityObjectBase asset)
		{
			return asset.SerializedFile.Collection.IsScene(asset.SerializedFile) ||
			       PrefabExportCollection.IsValidAsset(asset);
		}

		public override IExportCollection CreateCollection(VirtualSerializedFile virtualFile, IUnityObjectBase asset)
		{
			if (asset.SerializedFile.Collection.IsScene(asset.SerializedFile))
			{
				return new SceneExportCollection(this, asset.SerializedFile);
			}
			else if (PrefabExportCollection.IsValidAsset(asset))
			{
				return new PrefabExportCollection(this, virtualFile, asset);
			}
			else
			{
				return new FailExportCollection(this, asset);
			}
		}
	}
}
