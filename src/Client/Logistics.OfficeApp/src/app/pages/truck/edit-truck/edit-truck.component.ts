import {Component, OnInit, ViewEncapsulation} from '@angular/core';
import {NgIf} from '@angular/common';
import {FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule} from '@angular/forms';
import {ActivatedRoute, RouterLink} from '@angular/router';
import {ConfirmationService, MessageService} from 'primeng/api';
import {ButtonModule} from 'primeng/button';
import {AutoCompleteModule} from 'primeng/autocomplete';
import {ProgressSpinnerModule} from 'primeng/progressspinner';
import {CardModule} from 'primeng/card';
import {ConfirmDialogModule} from 'primeng/confirmdialog';
import {ToastModule} from 'primeng/toast';
import {CreateTruck, Employee, UpdateTruck} from '@core/models';
import {ApiService} from '@core/services';
import {NumberUtils} from '@shared/utils';


@Component({
  selector: 'app-edit-truck',
  templateUrl: './edit-truck.component.html',
  styleUrls: ['./edit-truck.component.scss'],
  encapsulation: ViewEncapsulation.None,
  standalone: true,
  imports: [
    ToastModule,
    ConfirmDialogModule,
    CardModule,
    NgIf,
    ProgressSpinnerModule,
    FormsModule,
    ReactiveFormsModule,
    AutoCompleteModule,
    ButtonModule,
    RouterLink,
  ],
  providers: [
    ConfirmationService,
  ],
})
export class EditTruckComponent implements OnInit {
  public id: string | null;
  public headerText: string;
  public isBusy: boolean;
  public editMode: boolean;
  public form: FormGroup;
  public suggestedDrivers: Employee[];

  constructor(
    private apiService: ApiService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private route: ActivatedRoute)
  {
    this.id = null;
    this.suggestedDrivers = [];
    this.headerText = 'Edit a truck';
    this.isBusy = false;
    this.editMode = true;
    this.form = new FormGroup({
      truckNumber: new FormControl(0, Validators.required),
      drivers: new FormControl([], Validators.required),
      driverIncomePercentage: new FormControl(0, Validators.compose([Validators.required, Validators.min(0), Validators.max(1)])),
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.id = params['id'];
    });

    if (!this.id) {
      this.editMode = false;
      this.headerText = 'Add a new truck';
      return;
    }

    this.fetchTruck(this.id);
  }

  searchDriver(event: any) {
    this.apiService.getDrivers(event.query).subscribe((result) => {
      if (result.isSuccess && result.items) {
        this.suggestedDrivers = result.items;
      }
    });
  }

  submit() {
    const drivers = this.form.value.drivers as Employee[];

    if (drivers.length === 0) {
      this.messageService.add({key: 'notification', severity: 'error', summary: 'Error', detail: 'Select a driver'});
      return;
    }

    if (this.editMode) {
      this.updateTruck();
    }
    else {
      this.createTruck();
    }
  }

  confirmToDelete() {
    this.confirmationService.confirm({
      message: 'Are you sure that you want to delete this truck?',
      accept: () => this.deleteTruck(),
    });
  }

  private fetchTruck(id: string) {
    this.apiService.getTruck(id).subscribe((result) => {
      if (result.isSuccess && result.value) {
        const truck = result.value;
        this.form.patchValue({
          truckNumber: truck.truckNumber,
          drivers: truck.drivers,
          driverIncomePercentage: NumberUtils.toPercentage(truck.driverIncomePercentage),
        });
      }
    });
  }

  private createTruck() {
    this.isBusy = true;
    const drivers = this.form.value.drivers as Employee[];

    const command: CreateTruck = {
      truckNumber: this.form.value.truckNumber,
      driverIncomePercentage: NumberUtils.toRatio(this.form.value.driverIncomePercentage),
      driverIds: drivers.map((i) => i.id),
    };

    this.apiService.createTruck(command).subscribe((result) => {
      if (result.isSuccess) {
        this.messageService.add({key: 'notification', severity: 'success', summary: 'Notification', detail: 'A new truck has been created successfully'});
        this.resetForm();
      }

      this.isBusy = false;
    });
  }

  private updateTruck() {
    this.isBusy = true;
    const drivers = this.form.value.drivers as Employee[];

    const updateTruckCommand: UpdateTruck = {
      id: this.id!,
      truckNumber: this.form.value.truckNumber,
      driverIncomePercentage: NumberUtils.toRatio(this.form.value.driverIncomePercentage),
      driverIds: drivers.map((i) => i.id),
    };

    this.apiService.updateTruck(updateTruckCommand).subscribe((result) => {
      if (result.isSuccess) {
        this.messageService.add({key: 'notification', severity: 'success', summary: 'Notification', detail: 'Truck has been updated successfully'});
      }

      this.isBusy = false;
    });
  }

  private deleteTruck() {
    if (!this.id) {
      return;
    }

    this.isBusy = true;
    this.apiService.deleteTruck(this.id).subscribe((result) => {
      if (result.isSuccess) {
        this.messageService.add({key: 'notification', severity: 'success', summary: 'Notification', detail: 'A truck has been deleted successfully'});
        this.resetForm();
      }

      this.isBusy = false;
    });
  }

  private resetForm() {
    this.editMode = false;
    this.headerText = 'Add a new truck';
    this.id = null;
  }
}
