import {Component, OnInit, ViewContainerRef} from '@angular/core';
import {ApiService} from "../../_services/api.service";
import {firstValueFrom} from "rxjs";

import {NzModalService} from 'ng-zorro-antd/modal';
import {NzMessageService} from 'ng-zorro-antd/message';

import {CreateContainerFormComponent} from "../../_forms/create-form/create-container-form.component";
import {Container} from "../../_models/container";
import {NzSafeAny} from "ng-zorro-antd/core/types";
import {ModalBuilderService} from "../../_services/modal-builder.service";
import {CreateShipFormComponent} from "../../_forms/create-form/create-ship-form.component";

@Component({
  selector: 'app-containers',
  templateUrl: './containers.component.html',
  styleUrls: ['./containers.component.css'],
  providers: [ModalBuilderService]
})
export class ContainersComponent implements OnInit {

  containers: Container[] = [];

  isLoading = false

  constructor(private message: NzMessageService,
              private apiService: ApiService,
              private modalBuilder: ModalBuilderService,
              viewContainerRef: ViewContainerRef) {
    modalBuilder.init(viewContainerRef)
  }

  async ngOnInit() {
    this.isLoading = true;
    this.containers = await this.apiService.getContainers(1, 1000);
    this.isLoading = false;
  }

  showCreateModal(): void {

    const onSave = async (componentInstance: CreateContainerFormComponent) =>  {
      if (!componentInstance.isValid())
        return;

      const request = componentInstance.prepareForApi();
      return this.apiService
        .addContainer(request)
        .then((containerId) => {
          this.message.create('success', `The container has been created!`);
          this.addToList({ id: +containerId, name: request.name })
        })
    }

    this.modalBuilder.showModal('Create Container', CreateContainerFormComponent, onSave, null, '250px')
  }

  private addToList(item: Container) {
    this.containers = [... this.containers, item];
  }
}
