namespace SubmissionProcessor.Worker.Settings
{
    public sealed class RabbitMQSettings
    {
        public string Host { get; set; } = string.Empty ;
        public int Port { get; set; } 

        public string VirtualHost { get; set; } = string.Empty ;

        public string Username { get; set; } = string.Empty ;
        public string Password { get; set; } = string.Empty ;

        public string SubmissionProcessingQueueName { get; set; } = "submission-processing" ;

        public int MaxRetryAttempts { get; } = 3 ;

        public string DlxName { get; set; } = "submission-processing-dead-letter-exchange";

        public string DlqName { get; set; } = "submission-processing-failed" ;

    }
}