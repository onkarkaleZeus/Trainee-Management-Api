
namespace TraineeManagement.Api.DTOs.TraineeDto
{
    public class PaginationTraineeResponse
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10 ;
        public int TotalRecords { get; set; }
        public IEnumerable<TraineeResponse> Data { get; set; } = [] ;

    }
}