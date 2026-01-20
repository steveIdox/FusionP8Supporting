using Sword.Fusion.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sword.BusinessApi.Lifecycles;
using Sword.Fusion.Api.Objects;
using Sword.Fusion.Api.Contents;
using Sword.FusionP8.FileNetServices.Services;
using Sword.FusionP8.FileNetWrappers.Apiimpl.Mapping;
using Sword.Fusion.Core.Data;
using Sword.FusionP8.FileNetServices.LogicalDocuments;
using Sword.FusionP8.FileNetServices.ComponentRelationships;

namespace TransmittablePackageActivity
{
    internal class LT
    {
        public void CreateLogicalContentsForDocument(IDocument document, EntitiesCollection entitiesCollection, IDocument result)
        {
            //  NOTES
            //  in this IDocument document
            //  and Document initialDocument will be initially the same (But we want Document initialDocument to be a new Document object from a chosen class)
            //
            LogicalNodesCollectionBuilder logicalNodesCollectionBuilder = null;
            for (int i = 0; i < entitiesCollection.Count; i++)
            {
                Sword.FusionP8.FileNetServices.Documents.Document initialDocument = (Sword.FusionP8.FileNetServices.Documents.Document)entitiesCollection[i];
                LogicalDocumentContents contents = new RelationshipService().RetrieveLogicalContent(initialDocument);
                if (contents != null)
                {
                    LogicalDocumentContentsBuilder contentsBuilder = contents.GetBuilder();
                    contentsBuilder = contentsBuilder.CloneNew((Sword.FusionP8.FileNetServices.Documents.Document)document, FreezeType.Unfrozen);
                    if (logicalNodesCollectionBuilder == null)
                    {
                        logicalNodesCollectionBuilder = contentsBuilder.LogicalNodes;
                    }
                    else
                    {
                        foreach (LogicalNodeBuilder node in contentsBuilder.LogicalNodes)
                        {
                            logicalNodesCollectionBuilder.Add(node);
                            //logicalNodesCollectionBuilder.Add();
                        }
                    }
                }
            }
        }

        private void AddLogicalNodeLabel(Sword.FusionP8.FileNetServices.Documents.Document document)
        {
            ComponentRelationshipBuilder builder = new ComponentRelationshipBuilder(document.ObjectStore, document.ClassDefinition.Name);

        }
    }
}
