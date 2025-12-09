import { Routes } from '@angular/router';
import { InputScreenComponent } from './components/input-screen/input-screen.component';
import { DisplayScreenComponent } from './components/display-screen/display-screen.component';
import { PaginationViewComponent } from './components/pagination-view/pagination-view.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'input',
    pathMatch: 'full'
  },
  {
    path: 'input',
    component: InputScreenComponent,
    title: 'מחולל קומבינציות - הכנסת נתונים'
  },
  {
    path: 'display',
    component: DisplayScreenComponent,
    title: 'מחולל קומבינציות - תצוגה'
  },
  {
    path: 'pagination',
    component: PaginationViewComponent,
    title: 'מחולל קומבינציות - כל הקומבינציות'
  },
  {
    path: '**',
    redirectTo: 'input'
  }
];
