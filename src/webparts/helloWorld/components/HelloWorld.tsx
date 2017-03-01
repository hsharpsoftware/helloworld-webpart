import * as React from 'react';
import styles from './HelloWorld.module.scss';
import { IHelloWorldProps } from './IHelloWorldProps';
import { escape } from '@microsoft/sp-lodash-subset';
import TodoApp = require("../../../Client/out/react-component");

export default class HelloWorld extends React.Component<IHelloWorldProps, void> {
  public render(): React.ReactElement<IHelloWorldProps> {
    return React.createElement(TodoApp.TodoApp, null, null );
  }

}
