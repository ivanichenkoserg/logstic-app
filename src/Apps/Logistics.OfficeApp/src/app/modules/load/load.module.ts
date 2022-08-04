import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@shared/shared.module';
import { LoadRoutingModule } from './load-routing.module';
import { ListLoadComponent } from './pages/list-load/list-load.component';
import { EditLoadComponent } from './pages/edit-load/edit-load.component';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    ListLoadComponent,
    EditLoadComponent
  ],
  imports: [
    CommonModule,
    LoadRoutingModule,
    SharedModule,
    ReactiveFormsModule
  ]
})
export class LoadModule { }
