import * as d3 from 'd3';

// load the data from a json file and create the d3 svg in the then function
export function createAlbumViz(dataUrl: string, svgElementId: string) {
  d3.json(dataUrl).then((data: any) => {
    const svg = d3.select(`#${svgElementId}`);
    const width = +svg.attr('width');
    const height = +svg.attr('height');
    const margin = { top: 20, right: 30, bottom: 40, left: 40 };

    const x = d3.scaleBand()
        .range([margin.left, width - margin.right])
        .padding(0.1);

    const y = d3.scaleLinear()
        .range([height - margin.bottom, margin.top]);

    x.domain(data.map((d: any) => d.Title));
    y.domain([0, d3.max(data, (d: any) => Number(d.Price))!]);
    svg.append('g')
        .attr('fill', 'steelblue')
      .selectAll('rect')
        .data(data)
      .enter().append('rect')
        .attr('x', (d: any) => x(d.Title)!)
        .attr('y', (d: any) => y(d.Price))
        .attr('height', (d: any) => y(0) - y(d.Price))
        .attr('width', x.bandwidth());
    svg.append('g')
        .attr('transform', `translate(0,${height - margin.bottom})`)
        .call(d3.axisBottom(x))
        .selectAll('text')
          .attr('transform', 'rotate(-45)')
          .style('text-anchor', 'end');
    svg.append('g')
        .attr('transform', `translate(${margin.left},0)`)
        .call(d3.axisLeft(y));
  });
}