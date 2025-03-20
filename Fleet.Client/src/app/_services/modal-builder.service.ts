import {Injectable, Type, ViewContainerRef} from "@angular/core";
import {NzMessageService} from "ng-zorro-antd/message";
import {NzSafeAny} from "ng-zorro-antd/core/types";
import {ConfirmType, NzModalService} from "ng-zorro-antd/modal";
import {UnloadTruckRequest} from "../_requests/UnloadTruckRequest";

@Injectable({
  providedIn: 'root'
})
export class ModalBuilderService {

  viewContainerRef!: ViewContainerRef

  constructor(private modal: NzModalService,
              private message: NzMessageService) {
  }

  init(viewContainerRef: ViewContainerRef){
    this.viewContainerRef = viewContainerRef;
  }

  showConfirmationModal(
    title: string,
    content: string,
    okText: string,
    onOk: () => Promise<NzSafeAny>,
  ) {
    const _self = this;
    this.modal.confirm({
      nzTitle: title,
      nzContent: content,
      nzOkText: okText,
      nzViewContainerRef: this.viewContainerRef,
      nzOnOk: (_) => {
        return onOk()
          .catch(e => _self.message.error(e.error.message));
      }
    });
  }

  showModal<T = NzSafeAny>(
    title: string,
    contentComponent: Type<T>,
    onSave: (contentComponentInstance: T) => Promise<NzSafeAny> | undefined,
    dataToPass?: any,
    width: string = '350px'
  ) {
    const _self = this;
    const modal = this.modal.create<T>({
      nzTitle: title,
      nzWidth: width,
      nzContent: contentComponent,
      nzViewContainerRef: this.viewContainerRef,
      nzData: dataToPass ?? {},
      nzFooter: [
        {
          label: 'Cancel',
          onClick(): NzSafeAny | Promise<NzSafeAny> {
            modal.destroy();
          }
        },
        {
          label: 'Save',
          onClick(contentComponentInstance: T): NzSafeAny | Promise<NzSafeAny> {
            const promise = onSave(contentComponentInstance);
            if (promise) {
              return promise
                .then(() => modal.destroy())
                .catch(e => _self.message.error(e.error.message));
            }
          }
        }
      ]
    });
  }
}
