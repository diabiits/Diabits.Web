export function create(container, path, loop, autoplay, renderer) {
    const anim = lottie.loadAnimation({
        container,
        path,
        loop,
        autoplay,
        renderer: renderer || "svg",
    });

    anim.setSpeed(0.7);

    return {
        dispose: () => anim.destroy()
    };
}
