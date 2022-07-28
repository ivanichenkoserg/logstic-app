import { AfterViewInit, Component, OnInit } from '@angular/core';
import { Employee } from '@shared/models/employee';
import { ApiClientService } from '@shared/services/api-client.service';
import { LazyLoadEvent } from 'primeng/api';

@Component({
  selector: 'app-list-employee',
  templateUrl: './list-employee.component.html',
  styleUrls: ['./list-employee.component.scss']
})
export class ListEmployeeComponent implements OnInit {
  employees: Employee[];
  isBusy: boolean;
  totalRecords: number;
  page: number;

  constructor(private apiService: ApiClientService) {
    this.employees = [];
    this.isBusy = false;
    this.totalRecords = 0;
    this.page = 1;
  }

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(event?: LazyLoadEvent) {
    if (this.page < 1) {
      this.page = 1;
    }

    this.isBusy = true;

    this.apiService.getEmployees(undefined, this.page, 2).subscribe(result => {
      if (result.success && result.items) {
        this.employees = result.items;
        this.totalRecords = result.ItemsCount!;
      }

      this.isBusy = false;
    });
  }
}
