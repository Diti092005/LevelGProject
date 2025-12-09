import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-page-navigation',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './page-navigation.component.html',
  styleUrls: ['./page-navigation.component.scss']
})
export class PageNavigationComponent {
  @Input() currentPage: string = "1";
  @Input() totalPages: number = 1;
  @Input() canGoNext: boolean = true;
  @Input() canGoPrevious: boolean = false;
  
  @Output() firstPage = new EventEmitter<void>();
  @Output() previousPage = new EventEmitter<void>();
  @Output() nextPage = new EventEmitter<void>();
  @Output() lastPage = new EventEmitter<void>();
  @Output() goToPage = new EventEmitter<number>();

  pageInput: number | null = null;

  onPageInputSubmit() {
    if (this.pageInput && this.pageInput >= 1 && this.pageInput <= this.totalPages && this.pageInput !== Number(this.currentPage)) {
      this.goToPage.emit(this.pageInput);
      this.pageInput = null;
    }
  }
}
