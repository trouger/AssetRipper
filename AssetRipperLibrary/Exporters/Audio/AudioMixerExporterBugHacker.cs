using AssetRipper.Core.Interfaces;
using AssetRipper.Core.IO;
using AssetRipper.Core.Project;
using AssetRipper.SourceGenerated.Classes.ClassID_245;
using AssetRipper.Yaml;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AssetRipper.Library.Exporters.Audio
{
	public static class AudioMixerExporterBugHacker
	{
		public static bool HackedExporter(IExportContainer container, IEnumerable<IUnityObjectBase> assets, string path)
		{
			using Stream fileStream = System.IO.File.Create(path);
			using InvariantStreamWriter streamWriter = new InvariantStreamWriter(fileStream, UTF8);
			YamlWriter writer = new YamlWriter();
			writer.WriteHead(streamWriter);
			foreach (IUnityObjectBase asset in assets)
			{
				YamlDocument doc = asset.ExportYamlDocument(container);
				// this is where the hack goes
				if (asset is IAudioMixerSnapshotController snapshot)
				{
					var root = doc.Root as YamlMappingNode;
					var props = root.m_children[0].Value as YamlMappingNode;
					foreach (var child in props.m_children)
					{
						if ((child.Key as YamlScalarNode)?.Value == "m_FloatValues")
						{
							var newMappingNode = new YamlMappingNode();
							var sequence = child.Value as YamlSequenceNode;
							foreach (YamlMappingNode mapping in sequence.m_children)
							{
								var guidNode = mapping.m_children[0].Value;
								var valueNode = mapping.m_children[1].Value;
								newMappingNode.Add(guidNode, valueNode);
							}
							var newKeyValuePair = new KeyValuePair<YamlNode, YamlNode>(child.Key, newMappingNode);
							int index = props.m_children.IndexOf(child);
							props.m_children.RemoveAt(index);
							props.m_children.Insert(index, newKeyValuePair);
							break;
						}
					}
					
					foreach (var child in props.m_children)
					{
						if ((child.Key as YamlScalarNode)?.Value == "m_TransitionOverrides")
						{
							var newMappingNode = new YamlMappingNode();
							var sequence = child.Value as YamlSequenceNode;
							foreach (YamlMappingNode mapping in sequence.m_children)
							{
								var guidNode = mapping.m_children[0].Value;
								var valueNode = mapping.m_children[1].Value;
								newMappingNode.Add(guidNode, valueNode);
							}
							var newKeyValuePair = new KeyValuePair<YamlNode, YamlNode>(child.Key, newMappingNode);
							int index = props.m_children.IndexOf(child);
							props.m_children.RemoveAt(index);
							props.m_children.Insert(index, newKeyValuePair);
							break;
						}
					}
				}
				writer.WriteDocument(doc);
			}

			writer.WriteTail(streamWriter);
			return true;
		}
		
		private static readonly Encoding UTF8 = new UTF8Encoding(false);
	}
}
