using Microsoft.Extensions.VectorData;

namespace OrionLumina.Application.CloudToEmbeddings
{
    public class Credentials
    {

        public string Client_Id { get; init; } = "";

        public string Project_Id { get; init; } = "";

        public string Auth_Uri { get; init; } = "";

        public string Token_Uri { get; init; } = "";

        public string Auth_Provider_X509_Cert_Url { get; init; } = "";

        public string Client_Secret { get; init; } = "";


    }

    public class Movie
    {
        [VectorStoreRecordKey]
        public int Key { get; set; }

        [VectorStoreRecordData]
        public string Title { get; set; }

        [VectorStoreRecordData]
        public string Description { get; set; }

        [VectorStoreRecordVector(384, DistanceFunction.CosineSimilarity)]
        public ReadOnlyMemory<float> Vector { get; set; }
    }
}
