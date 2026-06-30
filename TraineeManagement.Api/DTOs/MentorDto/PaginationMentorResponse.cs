namespace TraineeManagement.Api.DTOs.MentorDto
{
    public class PaginationMentorResponse
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10 ;
        public int TotalRecords { get; set; }
        public IEnumerable<MentorResponse> Data { get; set; } = [] ;

    }
}