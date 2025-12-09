export interface PaginatedPermutationResponse {
  permutations: number[][];
  startSequenceNumber: number;
  pageSize: number;
  totalPermutations: number;
  message: string;
  success: boolean;
}
