import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { PermutationService } from '../../services/permutation.service';
import { StateService } from '../../services/state.service';

@Component({
  selector: 'app-input-screen',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './input-screen.component.html',
  styleUrl: './input-screen.component.scss'
})
export class InputScreenComponent {
  n: number | null = null;
  loading = false;

  private readonly factorialCache: number[] = [
    1,                           
    1,                           
    2,                           
    6,                      
    24,                         
    120,                       
    720,                       
    5040,                      
    40320,                    
    362880,                  
    3628800,                     
    39916800,                    
    479001600,                   
    6227020800,                 
    87178291200,                 
    1307674368000,               
    20922789888000,              
    355687428096000,             
    6402373705728000,            
    121645100408832000,          
    2432902008176640000          
  ];

  constructor(
    private permutationService: PermutationService,
    private stateService: StateService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  calculateFactorial(n: number): number {
    if (n < 0 || n > 20) return 0;
    return this.factorialCache[n];
  }

  onStart(): void {
    if (!this.n || this.n < 1 || this.n > 20) {
      this.snackBar.open('אנא הכנס מספר תקין בין 1 ל-20', 'סגור', { duration: 3000 });
      return;
    }

    this.loading = true;
    this.permutationService.startSession(this.n).subscribe({
      next: (response) => {
        this.loading = false;
        this.stateService.setSessionData(response);
        this.stateService.setCurrentPermutation(null);
        this.router.navigate(['/display']);
      },
      error: (error) => {
        this.loading = false;
        this.snackBar.open('שגיאה ביצירת סשן', 'סגור', { duration: 3000 });
        console.error('Error starting session:', error);
      }
    });
  }
}
