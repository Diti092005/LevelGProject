import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatChipsModule } from '@angular/material/chips';
import { PermutationService } from '../../services/permutation.service';
import { StateService } from '../../services/state.service';
import { StartSessionResponse, PermutationResponse } from '../../models';
import { PageNavigationComponent } from '../page-navigation/page-navigation.component';

@Component({
  selector: 'app-pagination-view',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatChipsModule,
    PageNavigationComponent
  ],
  templateUrl: './pagination-view.component.html',
  styleUrl: './pagination-view.component.scss'
})
export class PaginationViewComponent implements OnInit {

  sessionData: StartSessionResponse | null = null;
  currentPermutation: PermutationResponse | null = null;
  permutations: number[][] = [];
  loading = false;
  loadingMessage = 'טוען קומבינציות...';
  pageSize: number = 20;
  previousPageSize: number = 20;
  startIndex = 0;
  currentPage :string= "1";

  constructor(
    private permutationService: PermutationService,
    private stateService: StateService,
    private router: Router,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    let sessionLoaded = false;
    let permutationLoaded = false;

    this.stateService.sessionData$.subscribe(data => {
      this.sessionData = data;
      if (!data) {
        this.router.navigate(['/input']);
        return;
      }
      sessionLoaded = true;
      if (permutationLoaded && sessionLoaded && this.currentPermutation) {
        this.loadPermutations();
      }
    });

    this.stateService.currentPermutation$.subscribe(perm => {
      this.currentPermutation = perm;
      if (perm && perm.sequenceNumber) {
        this.startIndex = perm.sequenceNumber;
      } else {
        this.startIndex = 0;
      }

      permutationLoaded = true;
      if (sessionLoaded && this.sessionData && this.permutations.length === 0) {
        this.loadPermutations();
      }
    });
  }

  loadPermutations(): void {
    this.fetchPermutations(this.startIndex);
  }

  private handlePermutationsResponse(response: any): void {
    this.loading = false;
    if (response.success && response.permutations.length > 0) {
      this.permutations = response.permutations;
      this.startIndex = response.startSequenceNumber;
      this.previousPageSize = this.pageSize;
      this.updateCurrentPage();
    } else {
      this.snackBar.open(response.message || 'אין תוצאות להצגה', 'סגור', { duration: 3000 });
    }
  }

  private updateCurrentPage(): void {
    this.currentPage = (Math.floor(this.startIndex / this.pageSize) + 1).toString();
  }

  get totalPages(): number {
    if (!this.sessionData?.totalPermutations) return 1;
    return Math.ceil(this.sessionData.totalPermutations / this.pageSize);
  }

  onFirstPage(): void {
    if (this.startIndex === 0) return;
    this.fetchPermutations(0);
  }

  onPreviousPage(): void {
    if (!this.canGoToPreviousPage()) return;
    const newIndex = Math.max(0, this.startIndex - this.pageSize);
    this.fetchPermutations(newIndex);
  }

  onNextPage(): void {
    if (!this.sessionData?.sessionId || !this.canGoToNextPage()) return;

    const currentStart = this.startIndex;
    const currentSize = Number(this.pageSize);
    const newIndex = currentStart + currentSize;
    
    this.fetchPermutations(newIndex);
  }

  onPageSizeChange(): void {
    const newPageSize = Number(this.pageSize);
    
    if (newPageSize <= this.previousPageSize && this.permutations.length >= newPageSize) {
      this.permutations = this.permutations.slice(0, newPageSize);
      this.previousPageSize = newPageSize;
      this.updateCurrentPage();
    } else {
      const lastDisplayedIndex = this.startIndex + this.permutations.length;
      if (this.sessionData && lastDisplayedIndex >= this.sessionData.totalPermutations) {
        this.snackBar.open('אין עוד קומבינציות להציג - זו הקומבינציה האחרונה', 'סגור', { duration: 3000 });
        this.pageSize = this.previousPageSize; // Revert to previous page size
        return;
      }
      
      this.snackBar.open(`טוען ${newPageSize} פריטים בדף...`, '', { duration: 2000 });
      this.loadingMessage = `טוען ${newPageSize} קומבינציות...`;
      this.previousPageSize = newPageSize;
      this.fetchPermutations(this.startIndex);
    }
  }

  private fetchPermutations(newIndex: number): void {
    if (!this.sessionData?.sessionId) return;

    this.loading = true;
    if (!this.loadingMessage.includes('קומבינציות')) {
      this.loadingMessage = 'טוען קומבינציות...';
    }

    this.permutationService.getAll(this.sessionData.sessionId, this.pageSize, newIndex).subscribe({
      next: (response) => this.handlePermutationsResponse(response),
      error: (error) => {
        this.loading = false;
        this.snackBar.open('שגיאה בטעינת קומבינציות', 'סגור', { duration: 3000 });
        console.error('Error loading permutations:', error);
      }
    });
  }

  onBack(): void {
    if (this.permutations.length > 0 && this.sessionData) {
      const lastPermutation = this.permutations[this.permutations.length - 1];
      const lastSequenceNumber = this.startIndex + this.permutations.length;

      this.stateService.setCurrentPermutation({
        permutation: lastPermutation,
        sequenceNumber: lastSequenceNumber,
        success: true,
        message: ''
      });
    }
    this.router.navigate(['/display']);
  }

  canGoToNextPage(): boolean {
    if (!this.sessionData || this.permutations.length === 0) return false;
    const lastDisplayedIndex = this.startIndex + this.permutations.length;
    return lastDisplayedIndex < this.sessionData.totalPermutations;
  }

  canGoToPreviousPage(): boolean {
    return this.startIndex > 0;
  }

  onGoToPage(pageNumber: number): void {
    const newIndex = (pageNumber - 1) * this.pageSize;
    if (newIndex >= 0 && this.sessionData && newIndex < this.sessionData.totalPermutations) {
      this.fetchPermutations(newIndex);
    }
  }

  goToLastPage(): void {
    if (!this.sessionData?.sessionId || !this.canGoToNextPage()) return;

    const totalPermutations = this.sessionData.totalPermutations || 0;

    if (!Number.isSafeInteger(totalPermutations)) {
      this.snackBar.open('המספר גדול מדי - השתמש בכפתור "הבא" במקום', 'סגור', { duration: 3000 });
      return;
    }

    const newIndex = totalPermutations - this.pageSize;

    if (newIndex < 0) {
      return;
    }

    this.fetchPermutations(newIndex);
  }
}
