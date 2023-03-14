import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';
import NavigationBar from "./NavigationBar";

export class Layout extends Component {
    static displayName = Layout.name;

    render() {
        const menuItems = [
            { label: "Home", link: "/" },
            { label: "Counter", link: "/counter" },
            { label: "Fetch data", link: "/fetch-data" },
            { label: "Login", link: "/Login" }
        ];

        return (
            <div>
                <NavigationBar menuItems={menuItems} />
                <Container>
                    {this.props.children}
                </Container>
            </div>
        );
    }
}
