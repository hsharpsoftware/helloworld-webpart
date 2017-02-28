import * as React from 'react';
import styles from './HelloWorld.module.scss';
import { IHelloWorldProps } from './IHelloWorldProps';
import { escape } from '@microsoft/sp-lodash-subset';
import * as bundle from '../../../Client/out/bundle';

export default class HelloWorld extends React.Component<IHelloWorldProps, void> {
  public render(): React.ReactElement<IHelloWorldProps> {
    return (
      <bundle.TodoApp />
    );
  }
}
