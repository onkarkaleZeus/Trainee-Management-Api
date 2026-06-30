namespace TraineeManagement.Api.DTOs.LearningTaskDto
{
    public class PaginationTaskResponse
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10 ;
        public int TotalRecords { get; set; }
        public IEnumerable<LearningTaskResponse> Data { get; set; } = [] ;

    }
}