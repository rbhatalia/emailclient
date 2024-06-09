using EmailManagement.Common;
using Nest;
using System;

public class ElasticsearchService
{
    private readonly IElasticClient _client;

    public ElasticsearchService(string uri)
    {
        var settings = new ConnectionSettings(new Uri(uri))
            .DefaultMappingFor<EmailMessage>(m => m.IndexName(Constants.MAILMESSAGES_INDEX_NAME)); ;
        _client = new ElasticClient(settings);
    }

    public void IndexEmailMessage(string indexName, EmailMessage emailMessage)
    {
        _client.Index(emailMessage, idx => idx.Index(indexName));
    }

    public void IndexMailboxDetail(string indexName, MailboxDetail mailboxDetail)
    {
        _client.Index(mailboxDetail, idx => idx.Index(indexName));
    }

    public async Task<List<EmailMessage>> GetEmailMessages(string indexName, string emailId)
    {
        var searchResponse = await _client.SearchAsync<EmailMessage>(s => s
            .Index(indexName)
            .Query(q => q.Match(m => m.Field(f => f.EmailId).Query(emailId)))
            .Size(10) // Limit to 10 results (optional)
        );

        return searchResponse.Documents.ToList();
    }

    public void CreateIndices()
    {
        _client.Indices.Create(Constants.MAILMESSAGES_INDEX_NAME, c => c
            .Map<EmailMessage>(m => m.AutoMap())
        );

        _client.Indices.Create(Constants.MAILBOX_INDEX_NAME, c => c
            .Map<MailboxDetail>(m => m.AutoMap())
        );
    }
}
