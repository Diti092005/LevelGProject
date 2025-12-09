import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  PermutationRequest, 
  PermutationResponse, 
  PaginatedPermutationResponse,
  StartSessionResponse 
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class PermutationService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  startSession(n: number): Observable<StartSessionResponse> {
    return this.http.post<StartSessionResponse>(`${this.apiUrl}/permutation/start`, { n });
  }

  getNext(sessionId: string): Observable<PermutationResponse> {
    return this.http.post<PermutationResponse>(`${this.apiUrl}/permutation/next`, { sessionId });
  }

  getAll(sessionId: string, pageSize: number, startIndex: number = 0): Observable<PaginatedPermutationResponse> {
    return this.http.post<PaginatedPermutationResponse>(`${this.apiUrl}/permutation/all`, { 
      sessionId, 
      pageSize,
      startIndex
    });
  }

  getPrevious(sessionId: string, currentIndex: number, pageSize: number): Observable<PaginatedPermutationResponse> {
    const startIndex = Math.max(0, currentIndex - pageSize);
    return this.http.post<PaginatedPermutationResponse>(`${this.apiUrl}/permutation/all`, { 
      sessionId, 
      pageSize,
      startIndex
    });
  }

  getFirstPage(sessionId: string, pageSize: number): Observable<PaginatedPermutationResponse> {
    return this.http.post<PaginatedPermutationResponse>(`${this.apiUrl}/permutation/all`, { 
      sessionId, 
      pageSize,
      startIndex: 0
    });
  }

  reset(sessionId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/permutation/reset`, { sessionId });
  }
}
