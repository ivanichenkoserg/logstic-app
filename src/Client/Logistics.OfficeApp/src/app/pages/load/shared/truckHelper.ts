import {ApiService} from '@core/services';
import {Observable, map} from 'rxjs';


export class TruckHelper {
  static findTruckDrivers(apiService: ApiService, searchQuery: string): Observable<TruckData[]> {
    return apiService.getTruckDrivers(searchQuery).pipe(
        map((result) => {
          if (!result.success || !result.items) {
            return [];
          }

          return result.items.map((truckDriver) => ({
            truckId: truckDriver.truck.id,
            driversName: TruckHelper.formatDriversName(truckDriver.truck.truckNumber, truckDriver.drivers.map((i) => i.fullName)),
          }));
        }),
    );
  }

  static formatDriversName(truckNumber: string, driversName: string[]): string {
    const formattedDriversName = driversName.join(',');
    return `${truckNumber} - ${formattedDriversName}`;
  }
}

export interface TruckData {
  driversName: string,
  truckId: string;
}
