import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { StartSessionResponse, PermutationResponse } from '../models';
import { PermutationService } from './permutation.service';

@Injectable({
  providedIn: 'root'
})
export class StateService {
  private readonly SESSION_STORAGE_KEY = 'permutation_session';
  private readonly PERMUTATION_STORAGE_KEY = 'permutation_current';
  private permutationService = inject(PermutationService);

  private sessionDataSubject = new BehaviorSubject<StartSessionResponse | null>(this.loadSessionFromStorage());
  private currentPermutationSubject = new BehaviorSubject<PermutationResponse | null>(this.loadPermutationFromStorage());

  sessionData$: Observable<StartSessionResponse | null> = this.sessionDataSubject.asObservable();
  currentPermutation$: Observable<PermutationResponse | null> = this.currentPermutationSubject.asObservable();

  private loadSessionFromStorage(): StartSessionResponse | null {
    try {
      const stored = localStorage.getItem(this.SESSION_STORAGE_KEY);
      return stored ? JSON.parse(stored) : null;
    } catch {
      return null;
    }
  }

  private loadPermutationFromStorage(): PermutationResponse | null {
    try {
      const stored = localStorage.getItem(this.PERMUTATION_STORAGE_KEY);
      return stored ? JSON.parse(stored) : null;
    } catch {
      return null;
    }
  }

  get sessionData(): StartSessionResponse | null {
    return this.sessionDataSubject.value;
  }

  get currentPermutation(): PermutationResponse | null {
    return this.currentPermutationSubject.value;
  }

  setSessionData(data: StartSessionResponse | null): void {
    if (data) {
      localStorage.setItem(this.SESSION_STORAGE_KEY, JSON.stringify(data));
    } else {
      localStorage.removeItem(this.SESSION_STORAGE_KEY);
    }
    this.sessionDataSubject.next(data);
  }

  setCurrentPermutation(permutation: PermutationResponse | null): void {
    if (permutation) {
      localStorage.setItem(this.PERMUTATION_STORAGE_KEY, JSON.stringify(permutation));
    } else {
      localStorage.removeItem(this.PERMUTATION_STORAGE_KEY);
    }
    this.currentPermutationSubject.next(permutation);
  }

  clearState(): void {
    const sessionId = this.sessionDataSubject.value?.sessionId;
    
    localStorage.removeItem(this.SESSION_STORAGE_KEY);
    localStorage.removeItem(this.PERMUTATION_STORAGE_KEY);
    this.sessionDataSubject.next(null);
    this.currentPermutationSubject.next(null);

    if (sessionId) {
      this.permutationService.reset(sessionId).subscribe({
        error: (err) => console.error('Failed to reset server session:', err)
      });
    }
  }
}
