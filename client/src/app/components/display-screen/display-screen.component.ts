import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatChipsModule } from '@angular/material/chips';
import { PermutationService } from '../../services/permutation.service';
import { StateService } from '../../services/state.service';
import { PermutationResponse, StartSessionResponse } from '../../models';

@Component({
  selector: 'app-display-screen',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatChipsModule
  ],
  templateUrl: './display-screen.component.html',
  styleUrl: './display-screen.component.scss'
})
export class DisplayScreenComponent implements OnInit {
  sessionData: StartSessionResponse | null = null;
  currentPermutation: PermutationResponse | null = null;
  loading = false;

  constructor(
    private permutationService: PermutationService,
    private stateService: StateService,
    private router: Router,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.stateService.sessionData$.subscribe(data => {
      this.sessionData = data;
    });
    
      this.stateService.currentPermutation$.subscribe(perm => {
        this.currentPermutation = perm;
      });
  }

  onNext(): void {
    if (!this.sessionData?.sessionId) return;
    if (this.loading) return; // Prevent multiple clicks

    this.loading = true;
    this.permutationService.getNext(this.sessionData.sessionId).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.currentPermutation = response;
          this.stateService.setCurrentPermutation(response);
        } else {
          this.snackBar.open(response.message, 'סגור', { duration: 3000 });
        }
      },
      error: (error) => {
        this.loading = false;
        this.snackBar.open('שגיאה בקבלת קומבינציה', 'סגור', { duration: 3000 });
        console.error('Error getting next permutation:', error);
      }
    });
  }

  onReset(): void {
    this.stateService.clearState();
    this.router.navigate(['/input']);
  }

  onShowAll(): void {
    if (!this.sessionData?.sessionId) return;
    this.router.navigate(['/pagination']);
  }

  isLastPermutation(): boolean {
    if (!this.currentPermutation || !this.sessionData) return false;
    return this.currentPermutation.sequenceNumber >= this.sessionData.totalPermutations;
  }

  isFirstPermutation(): boolean {
    if (!this.currentPermutation) return true;
    return this.currentPermutation.sequenceNumber === 1;
  }

  canGoNext(): boolean {
    return !this.isLastPermutation();
  }
}
