import {Component, OnInit, ViewContainerRef} from '@angular/core';
import {ApiService} from "../../_services/api.service";
import {firstValueFrom} from "rxjs";

import {NzModalService} from 'ng-zorro-antd/modal';
import {NzMessageService} from 'ng-zorro-antd/message';

import {CreateContainerFormComponent} from "../../_forms/create-form/create-container-form.component";
import {Container} from "../../_models/container";
import {NzSafeAny} from "ng-zorro-antd/core/types";

@Component({
  selector: 'app-containers',
  templateUrl: './containers.component.html',
  styleUrls: ['./containers.component.css']
})
export class ContainersComponent implements OnInit {

  containers: Container[] = [];

  isLoading = false

  constructor(private message: NzMessageService,
              private modal: NzModalService,
              private viewContainerRef: ViewContainerRef,
              private apiService: ApiService) {
  }

  async ngOnInit() {
    this.isLoading = true;
    this.containers = await this.apiService.getContainers(1, 1000);
    this.isLoading = false;
  }

  showCreateModal(): void {
    const modal = this.modal.create<CreateContainerFormComponent>({
      nzTitle: 'Create Container',
      nzWidth: '250px',
      nzContent: CreateContainerFormComponent,
      nzViewContainerRef: this.viewContainerRef,
      nzFooter: [
        {
          label: 'Cancel',
          onClick(): NzSafeAny | Promise<NzSafeAny> {
            modal.destroy();
          }
        },
        {
          label: 'Save',
          onClick: async (componentInstance: CreateContainerFormComponent) =>  {
            const isValid = componentInstance.validateForm();
            if (!isValid)
              return;

            const request = componentInstance.prepareForApi();
            await this.apiService
              .addContainer(request)
              .then((containerId) => {
                this.message.create('success', `The container has been created!`);
                this.addToList({ id: +containerId, name: request.name })
                modal.destroy();
              })
              .catch(e => {
                this.message.error(e.error.message)
              })
          }
        }
      ]
    });
  }

  private addToList(item: Container) {
    this.containers = [... this.containers, item];
  }
}
