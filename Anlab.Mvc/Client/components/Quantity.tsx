﻿import * as React from 'react';
import { NumberInput } from './numberInput/numberInput';
import { Input } from "react-toolbox/lib/input";

interface IQuantityProps {
    quantity?: number;
    onQuantityChanged: Function;
    quantityRef: any;
}

export class Quantity extends React.Component<IQuantityProps, any> {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <NumberInput
                numberRef={this.props.quantityRef}
                name='quantity'
                label='Quantity'
                value={this.props.quantity}
                onChanged={this.props.onQuantityChanged}
                integer
                min={1}
                max={100}
                required={true}
            />
        );
    }
}
